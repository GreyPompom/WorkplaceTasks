
export interface UserResponseDto {
  id: string;
  name: string;
  email: string;
  role: 'Admin' | 'Manager' | 'Member';
}