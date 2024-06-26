/// <binding BeforeBuild='min:js' />
var path = require('path'),
    vueCompiler = require('vue-template-compiler'),
    babel = require("@babel/core"),
    babelTemplate = require("@babel/template").default,
    codeFrameColumns = require('@babel/code-frame').codeFrameColumns,
    babelTypes = require("@babel/types"),
    through2 = require('through2'),
    uglify = require('gulp-uglify');

function vueCompile() {
    return through2.obj(function (file, _, callback) {
        var relativeFile = path.relative(file.cwd, file.path);
        var ext = path.extname(file.path);
        if (ext === '.vue') {
            var getComponent;
            getComponent = function (ast, sourceCode) {
                const ta = ast.program.body[0]
                if (!babelTypes.isExportDefaultDeclaration(ta)) {
                    var msg = 'Top level declaration in file ' + relativeFile + ' must be "export default {" \n' + codeFrameColumns(sourceCode, { start: ta.loc.start }, { highlightCode: true });
                    throw msg;
                }
                return ta.declaration;
            }

            var compile;
            compile = function (componentName, content) {
                var component = vueCompiler.parseComponent(content, []);
                if (component.styles.length > 0) {
                    component.styles.forEach(s => {
                        const linesToStyle = content.substr(0, s.start).split(/\r?\n/).length;
                        var msg = 'WARNING: <style> tag in ' + relativeFile + ' is ignored\n' + codeFrameColumns(content, { start: { line: linesToStyle } }, { highlightCode: true });
                        console.warn(msg);
                    });
                }

                var ast = babel.parse(component.script.content, {
                    parserOpts: {
                        sourceFilename: file.path
                    }
                });

                var vueComponent = getComponent(ast, component.script.content);
                vueComponent.properties.push(babelTypes.objectProperty(babelTypes.identifier('template'), babelTypes.stringLiteral(component.template.content)))

                var wrapInComponent = babelTemplate("Vue.component(NAME, COMPONENT);");
                var componentAst = wrapInComponent({
                    NAME: babelTypes.stringLiteral(componentName),
                    COMPONENT: vueComponent
                })

                ast.program.body = [componentAst]

                babel.transformFromAst(ast, null, null, function (err, result) {
                    if (err) {
                        callback(err, null)
                    }
                    else {
                        file.contents = Buffer.from(result.code);
                        callback(null, file)
                    }
                });
            }
            var componentName = path.basename(file.path, ext);
            if (file.isBuffer()) {
                compile(componentName, file.contents.toString());
            }
            else if (file.isStream()) {
                var chunks = [];
                file.contents.on('data', function (chunk) {
                    chunks.push(chunk);
                });
                file.contents.on('end', function () {
                    compile(componentName, Buffer.concat(chunks).toString());
                });
            }
        } else {
            callback(null, file);
        }
    });
}

// Gulp build script
var gulp = require("gulp"),
    sass = require('gulp-sass')(require('sass')),
    cssmin = require("gulp-cssmin"),
    concat = require("gulp-concat"),
    rename = require("gulp-rename"),
    uglifyes = require('uglify-es');

var css = [
    "resources/scss/link-field.scss",
];

var js = [
    {
        name: "link-field.js",
        items: [
            "resources/js/link-field.js",
            "resources/js/components/fields/link-field.vue",
            "resources/js/components/link-field-modal.vue"
        ]
    }]

var output = "assets/";

//
// Compile & minimize js files
//
gulp.task("min:js", function (done) {
    for (var n = 0; n < js.length; n++) {
        gulp.src(js[n].items, { base: "." })
            .pipe(vueCompile())
            .pipe(concat(output + "js/" + js[n].name))
            .pipe(gulp.dest("."))
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    }
    done();
});

//
// Compile & minimize CSS
//
gulp.task("min:css", function (done) {
    // Minimize and combine styles
    for (var n = 0; n < css.length; n++) {
        gulp.src(css[n])
            .pipe(sass().on("error", sass.logError))
            .pipe(gulp.dest(output + "css"))
            .pipe(cssmin())
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest(output + "css"));
    }
    done();
});

//
// Default tasks
//
gulp.task("build", gulp.parallel(["min:css", "min:js"]));
gulp.task("default", gulp.series("build"));
gulp.task("watch", function () {
    gulp.watch('resources/js/**/*', gulp.series('min:js'));
    gulp.watch('resources/scss/**/*', gulp.series('min:css'));
});