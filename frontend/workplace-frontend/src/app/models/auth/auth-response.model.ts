import { User } from './user.mode';

export interface AuthResponse {
  token: string;
  user: User;
}
