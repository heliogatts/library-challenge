namespace LibraryApi.Features.Genres.GetGenres;

public record GetGenresResponseItem(Guid Id, string Name, int BookCount);
