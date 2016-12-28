var gulp = require('gulp');

var sass = require('gulp-sass');
var cleanCss = require('gulp-clean-css')
var rename = require('gulp-rename')

gulp.task('bootstrap', function(){
	return gulp.src('static/lib/bootstrap/stylesheets/*.scss')
		.pipe(sass())
		.pipe(gulp.dest("static/lib/bootstrap/stylesheets/"))
		.pipe(cleanCss())
		.pipe(rename({
			suffix: ".min",
		}))
		.pipe(gulp.dest("static/lib/bootstrap/stylesheets/"))
});

gulp.task('sass', function(){
	return gulp.src('static/site/sass/*.scss')
		.pipe(sass())
		.pipe(gulp.dest("static/site/css/"))
		.pipe(cleanCss())
		.pipe(rename({
			suffix: ".min"
		}))
		.pipe(gulp.dest("static/site/css/"))
})

