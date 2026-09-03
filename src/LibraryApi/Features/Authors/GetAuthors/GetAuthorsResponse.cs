namespace LibraryApi.Features.Authors.GetAuthors;

public record GetAuthorsResponseItem(Guid Id, string Name, int BookCount);
