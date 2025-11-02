export interface UserUpdateDto {
  name: string;
  role: 'Admin' | 'Manager' | 'Member';
}