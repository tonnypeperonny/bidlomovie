﻿using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Lab._06.MVC.BL.DTO;
using Lab._06.MVC.BL.MovieService;
using Lab._06.MVC.Web.Models;
using PagedList;

namespace Lab._06.MVC.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService<MovieDto> movieService;

        public MovieController(IMovieService<MovieDto> movieService)
        {
            this.movieService = movieService;
        }

        public ActionResult GetAllMovies(int? page)
        {
            const int pageSize = 2;
            var pageNumber = page ?? 1;
            return View(movieService.AllMovies?.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        public ActionResult CreateMovie() => View();

        [HttpPost]
        [Authorize]
        public ActionResult CreateMovie(AddMovieModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                var img = new WebImage(upload.InputStream);
                if (img.Width > 700)
                {
                    img.Resize(700, 300);
                }

                var movieDto = new MovieDto
                {
                    MovieName = model.MovieName,
                    UserMovieNote = model.UserMovieNote,
                    MoviePoster = img.GetBytes()
                };
                movieService.Create(movieDto);
                return RedirectToAction("GetAllMovies", "Movie");
            }
            return View(model);
        }

        public ActionResult GetCurrentMovie(int movieid) => View(movieService.GetCurrentMovie(movieid));

        [Authorize]
        public ActionResult RemoveMovie(string movieName)
        {
            movieService.Remove(movieName);
            return RedirectToAction("GetAllMovies", "Movie");
        }

        [Authorize]
        public ActionResult UpdateMovie(int movieid)
        {
            var movie = movieService.GetCurrentMovie(movieid);
            var model = new AddMovieModel
            {
                MoviePoster = movie.MoviePoster,
                MovieName = movie.MovieName,
                UserMovieNote = movie.UserMovieNote,
                MovieId = movie.MovieId
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateMovie(AddMovieModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    var img = new WebImage(upload.InputStream);
                    if (img.Width > 700)
                    {
                        img.Resize(700, 300);
                    }

                    var movie = new MovieDto
                    {
                        MoviePoster = img.GetBytes(),
                        MovieName = model.MovieName,
                        UserMovieNote = model.UserMovieNote,
                        MovieId = model.MovieId
                    };
                    movieService.Update(movie);
                }
                else
                {
                    var movie = new MovieDto
                    {
                        MovieName = model.MovieName,
                        UserMovieNote = model.UserMovieNote,
                        MovieId = model.MovieId
                    };
                    movieService.Update(movie);
                }
            }
            return RedirectToAction("GetAllMovies", "Movie");
        }
    }
}