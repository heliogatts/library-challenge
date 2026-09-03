namespace LibraryApi.Features.Authors.GetAuthorById;

public record GetAuthorByIdResponse(Guid Id, string Name, int BookCount);
