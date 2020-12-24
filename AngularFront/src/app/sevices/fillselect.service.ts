import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { CitiesResponse, CountriesResponse, CountryInfoResponse, RaveResponse, StatesResponse } from '../common/response';
import { ApiHttpServiceInputParameters, BaseApiHttpService } from './basehttp.service';

@Injectable({
  providedIn: 'root'
})

export class FillselectService extends BaseApiHttpService {

  constructor(public parameters: ApiHttpServiceInputParameters) {
    super(parameters);
  }

  protected getUrl(): string {
    return this.basePath;
  }

  getCountries(showError = true): Observable<CountriesResponse> {
    const result = this.http.get<CountriesResponse>(this.getUrl() + 'getallcountries');
    return result.pipe(catchError((err: HttpErrorResponse) => {
      if (showError) {
        this.catchErrorHandler(err);
      }
      return throwError(err);
    }));
  }

  getStates(countryName: string, showError = true): Observable<StatesResponse> {
    const result = this.http.get<StatesResponse>(this.getUrl() + 'getallstates?countryName=' + countryName);
    return result.pipe(catchError((err: HttpErrorResponse) => {
      if (showError) {
        this.catchErrorHandler(err);
      }
      return throwError(err);
    }));
  }

  getCities(countryName: string, stateName: string, showError = true): Observable<CitiesResponse> {
    if (stateName) {
      const result = this.http.get<CitiesResponse>(this.getUrl() + 'getallcities?countryName=' + countryName +
        "&stateName=" + stateName);
      return result.pipe(catchError((err: HttpErrorResponse) => {
        if (showError) {
          this.catchErrorHandler(err);
        }
        return throwError(err);
      }));
    }
    else {
      const result = this.http.get<CitiesResponse>(this.getUrl() + 'getallcities?countryName=' + countryName);
      return result.pipe(catchError((err: HttpErrorResponse) => {
        if (showError) {
          this.catchErrorHandler(err);
        }
        return throwError(err);
      }));
    }
  }

  getCountryInfo(countryName: string, showError = true): Observable<CountryInfoResponse> {
    const result = this.http.get<CountryInfoResponse>(this.getUrl() + 'getcountryinfo?countryName=' + countryName);
    return result.pipe(catchError((err: HttpErrorResponse) => {
      if (showError) {
        this.catchErrorHandler(err);
      }
      return throwError(err);
    }));
  }

  getRaveJson(birthdate: string, birthtime: string, timezone: string, showError = true): Observable<RaveResponse> {
    const result = this.http.get<RaveResponse>(this.getUrl() + 'rave-chart?birthdate=' + birthdate + "T" + birthtime + "&timezone=" + timezone);
    return result.pipe(catchError((err: HttpErrorResponse) => {
      if (showError) {
        this.catchErrorHandler(err);
      }
      return throwError(err);
    }));
  }
}
