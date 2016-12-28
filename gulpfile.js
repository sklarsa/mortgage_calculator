var gulp = require('gulp');

var sass = require('gulp-sass');

gulp.task('bootstrap', function(){
	return gulp.src('static/lib/bootstrap/stylesheets/*.scss')
		.pipe(sass())
		.pipe(gulp.dest("static/lib/bootstrap/stylesheets/"))
});

