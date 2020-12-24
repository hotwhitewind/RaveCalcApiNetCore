import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { User } from '../common/authModels';
import { ApiHttpServiceInputParameters, BaseApiHttpService } from './basehttp.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService extends BaseApiHttpService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;

  constructor(public parameters: ApiHttpServiceInputParameters) {
    super(parameters);
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  protected getUrl(): string {
    return this.basePath;
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string, showError = true) {
    const result = this.http.post<any>(`${this.getUrl()}login`, { username, password })
    return result.pipe(map(user => {
      // login successful if there's a jwt token in the response
      if (user && user.result.token) {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(user.result));
        this.currentUserSubject.next(user.result);
      }
      return user.result;
    }),
      catchError((err: HttpErrorResponse) => {
        if (showError) {
          this.catchErrorHandler(err);
        }
        return throwError(err);
      }));
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}
