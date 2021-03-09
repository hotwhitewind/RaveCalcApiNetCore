import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, mapTo, tap } from 'rxjs/operators';
import { RefreshToken, Role, Tokens, User, UserLocal } from '../common/authModels';
import { TokensResponse } from '../common/response';
import { ApiHttpServiceInputParameters, BaseApiHttpService } from './basehttp.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService extends BaseApiHttpService {
  private currentUserSubject: BehaviorSubject<UserLocal>;
  public currentUser: Observable<UserLocal>;

  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly REFRESH_TOKEN = 'REFRESH_TOKEN';
  private loggedUser: string;

  constructor(public parameters: ApiHttpServiceInputParameters) {
    super(parameters);
    this.currentUserSubject = new BehaviorSubject<UserLocal>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  protected getUrl(): string {
    return this.basePath;
  }

  public get currentUserValue(): UserLocal {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string, showError = true) {
    const result = this.http.post<any>(`${this.getUrl()}login`, { username, password });
    return result.pipe(
      tap(user => this.doLoginUser(user)),
      mapTo(true),
      catchError((err: HttpErrorResponse) => {
        if (showError) {
          this.catchErrorHandler(err);
        }
        return throwError(err);
      }));
  }

  logout(showError = true): Observable<any> {
    // remove user from local storage to log user out
    const refreshToken = JSON.parse(this.getRefreshToken()) as RefreshToken;
    const result = this.http.post<RefreshToken>(`${this.getUrl()}logout_command`, refreshToken);
    return result.pipe(
      tap(() => this.doLogoutUser()),
      mapTo(true),
      catchError((err: HttpErrorResponse) => {
        if (showError) {
          this.catchErrorHandler(err);
        }
        return throwError(err);
      }));
  }

  getJwtToken() {
    return localStorage.getItem(this.JWT_TOKEN);
  }

  private doLoginUser(userResult: any) {
    if (userResult && userResult.result.tokens) {
      this.currentUserSubject.next(
        { userName: userResult.result.userName, role: this.getRoleFromToken(userResult.result.tokens) });
      this.storeTokens(userResult.result.tokens);
    }
  }

  private getRoleFromToken(tokens: Tokens): Role {
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(tokens.jwtToken);
    if (decodedToken) {
      return decodedToken['role'] as Role;
    }
    else
      return Role.User;
  }

  private doLogoutUser() {
    this.currentUserSubject.next(null);
    this.removeTokens();
  }

  refreshToken() {
    const refreshToken = JSON.parse(this.getRefreshToken()) as RefreshToken;
    return this.http.post<TokensResponse>(`${this.getUrl()}refresh_token`, refreshToken).
      pipe(tap((tokens: TokensResponse) => {
        if (tokens.result == null)
          this.logout();
        else
          this.storeJwtToken(tokens.result.jwtToken);
      })
     );
  }

  private getRefreshToken() {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }

  private storeJwtToken(jwt: string) {
    localStorage.setItem(this.JWT_TOKEN, jwt);
  }

  private storeTokens(tokens: Tokens) {
    localStorage.setItem(this.JWT_TOKEN, tokens.jwtToken);
    localStorage.setItem(this.REFRESH_TOKEN, JSON.stringify(tokens.refreshToken));
  }

  private removeTokens() {
    localStorage.removeItem(this.JWT_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
  }
}
