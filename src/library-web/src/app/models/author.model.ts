export interface AuthorItem {
  id: string;
  name: string;
  bookCount: number;
}

export interface CreateAuthorRequest {
  name: string;
}
