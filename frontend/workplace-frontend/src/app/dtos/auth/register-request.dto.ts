export interface RegisterRequestDto {
  name: string;
  email: string;
  password: string;
  role?: 'Admin' | 'Manager' | 'Member';
}
