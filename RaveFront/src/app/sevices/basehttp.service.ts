import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { IServiceParameters } from "../interfaces/data-service";
import { AuthenticationService } from "./authentication.service";
import { DialogService } from "./dialog.service";

export abstract class CommonService<TParameters extends IServiceParameters> {
  /**
   *
   */
  constructor(protected parameters: TParameters) {

  }

  protected catchErrorHandler(err: HttpErrorResponse) {
    if (err.error instanceof Object) {
      this.showError(err.error);
    } else if (err.error) {
      this.showErrorMessage([err.message]);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(`Backend returned code ${err.status}, body was: ${err.error}`);
    }
  }


  protected showError(obj: any) {
    const messages: string[] = [];
    if (obj.error) {
      this.fillMessages(obj, messages);
    }
    this.showErrorMessage(messages);
  }

  fillMessages(obj: any, messages: string[]) {
    if (obj.message && obj.message instanceof Array) {
      (obj.message as Array<string>).forEach(w => {
        messages.push(w);
      });
    } else if (obj.message) {
      messages.push(obj.message);
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
    if (messages && messages.length > 0) {
      console.log(messages);
      this.parameters.dialogService.alert(messages);
    }
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
