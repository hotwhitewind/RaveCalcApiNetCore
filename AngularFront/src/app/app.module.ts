import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserdateComponent } from './components/userdate/userdate.component';
import { AlertComponent } from './components/dialogs/alert/alert.component';
import { ErrorListComponent } from './components/dialogs/error-list/error-list.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { ApiHttpServiceInputParameters } from './sevices/basehttp.service';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ErrorInterceptor } from './helpers/error.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    UserdateComponent,
    AlertComponent,
    ErrorListComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgxJsonViewerModule,
    RouterModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ApiHttpServiceInputParameters],
  bootstrap: [AppComponent]
})

export class AppModule {
}
