import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { environment } from 'src/environments/environment';
import { CountriesResponse } from '../common/response';
import { IServiceParameters } from '../interfaces/data-service';
import { DialogService } from './dialog.service';

export abstract class CommonService<TParameters extends IServiceParameters> {
  /**
   *
   */
  constructor(protected parameters: TParameters) {

  }

  protected catchErrorHandler(err: HttpErrorResponse) {
    if (err.error instanceof Error) {
      this.showErrorMessage([err.error.message]);
    } else if (err.error) {
      this.showError(err.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(`Backend returned code ${err.status}, body was: ${err.error}`);
    }
  }


  protected showError(obj: any) {
    const messages: string[] = [];
    if (obj.errors) {
      this.fillMessages(obj.errors, messages);
    } else {
      this.fillMessages(obj, messages);
    }

    this.showErrorMessage(messages);
    // this.showErrorMessage("<div>"+ messages.join('<br/>')+"</div>");
  }

  fillMessages(obj: any, messages: string[]) {
    if (obj.errors && obj.errors instanceof Array) {
      (obj.errors as Array<string>).forEach(w => {
        messages.push(w);
      });
    } else if (obj.errors) {
      this.fillMessages(obj.errors, messages);
    } else {
      Object.keys(obj).forEach((key) => {
        const value = obj[key];
        if (value instanceof Object) {
          this.fillMessages(value, messages);
        } else if (value instanceof Array) {
          (value as Array<any>).forEach(q => {
            messages.push(q);
          });
        } else {
          messages.push(value);
        }
      });
    }
  }

  protected showErrorMessage(messages: string[]) {
/*      if (messages && messages.length > 0) {
      this.parameters.dialogService.alert(messages).subscribe();
   } */
  }

  protected showSuccessMessage(message: string) {
  }
}

@Injectable()
export class ServiceParameters implements IServiceParameters {
  constructor(public dialogService: DialogService) {

  }
}

@Injectable()
export class ApiHttpServiceInputParameters implements IServiceParameters {
  constructor(public http: HttpClient, public dialogService: DialogService) {

  }
}

export abstract class BaseApiHttpService extends CommonService<ApiHttpServiceInputParameters> {
  protected get http(): HttpClient {
    return this.parameters.http;
  }
  protected abstract getUrl(): string;

  protected get basePath(): string {
    return environment.apiUrl;
  }
}

@Injectable({
  providedIn: 'root'
})

export class FillselectService /* extends BaseApiHttpService */ {

  constructor(public http: HttpClient/* parameters: ApiHttpServiceInputParameters */) {
    /* super(parameters); */
  }

  protected getUrl(): string {
    //return this.basePath;
    return environment.apiUrl;
  }

  getCountries(showError = true): Observable<CountriesResponse> {
    const result = this.http.get<CountriesResponse>(this.getUrl() + 'getallcountries');
    return result;/* .pipe(catchError((err: HttpErrorResponse) => {
      if (showError) {
        this.catchErrorHandler(err);
      }
      return throwError(err);
    })); */
  }
}
