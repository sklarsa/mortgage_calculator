var gulp = require('gulp');

var sass = require('gulp-sass');
var cleanCss = require('gulp-clean-css')
var rename = require('gulp-rename')
var coffee = require('gulp-coffee')
var uglify = require('gulp-uglify')


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
});

gulp.task('coffee', function(){
	return gulp.src('static/site/coffee/*.coffee')
		.pipe(coffee())
		.pipe(gulp.dest("static/site/js/"))
		.pipe(uglify())
		.pipe(rename({
			suffix: ".min"
		}))
		.pipe(gulp.dest("static/site/js/"))
});
