var gulp        = require('gulp');
var browserSync = require('browser-sync').create();
var sass        = require('gulp-sass');
var prefix      = require('gulp-autoprefixer');
var plumber     = require ('gulp-plumber');


// Static Server + watching scss/html files
gulp.task('serve', ['sass'], function() {

    browserSync.init({
        //server: "dist/"
        proxy: "localhost:4200"
    });

    gulp.watch("scss/*.scss", ['sass']);
    gulp.watch("**/*.html").on('change', browserSync.reload);
});

// Compile sass into CSS & auto-inject into browsers
gulp.task('sass', function() {
    return gulp.src("scss/*.scss")
        .pipe(plumber())
        .pipe(sass())
        .pipe(prefix(['last 15 versions', '> 1%', 'ie 8', 'ie 7'], { cascade: true }))
        .pipe(gulp.dest("dist/css"))
        .pipe(browserSync.stream())
        .pipe(gulp.dest("src/css"));
});

gulp.task('default', ['serve']);
