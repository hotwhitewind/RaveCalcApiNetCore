import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserdateComponent } from './components/userdate/userdate.component';
import { AlertComponent } from './components/dialogs/alert/alert.component';
import { ErrorListComponent } from './components/dialogs/error-list/error-list.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { ApiHttpServiceInputParameters } from './sevices/fillselect.service';

@NgModule({
  declarations: [
    AppComponent,
    UserdateComponent,
    AlertComponent,
    ErrorListComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgxJsonViewerModule
  ],
  providers: [ApiHttpServiceInputParameters],
  bootstrap: [AppComponent]
})

export class AppModule {
}
