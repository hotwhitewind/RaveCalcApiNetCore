export enum Role {
  Admin = 'admin',
  User = 'user'
}

export class User{
  userName: string;
  role: Role;
  token:string;
}
