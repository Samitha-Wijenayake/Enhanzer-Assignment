import { LocationDto } from './location.model';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  username: string;
  locations: LocationDto[];
}

export interface AuthSession {
  token: string;
  expiresAtUtc: string;
  username: string;
}
