namespace LibraryApi.Features.Genres.GetGenreById;

public record GetGenreByIdResponse(Guid Id, string Name, int BookCount);
