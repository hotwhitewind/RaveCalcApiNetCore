export enum Role {
  Admin = 'admin',
  User = 'user'
}

export class User{
  userName: string;
  tokens:Tokens;
}

export class UserLocal{
  userName: string;
  role: Role;
}

export class Tokens {
  jwtToken: string;
  refreshToken: RefreshToken;
}

export class RefreshToken{
  token: string;
  expires: Date;
  isExpired: boolean;
  created: Date;
  createdByIp: string;
  revoked: Date;
  revokedByIp: string;
  replacedByToken: string;
  isActive: boolean;
}