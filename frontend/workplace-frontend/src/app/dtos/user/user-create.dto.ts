export interface UserCreateDto {
  name: string;
  email: string;
  password: string;
  role: 'Admin' | 'Manager' | 'Member';
}