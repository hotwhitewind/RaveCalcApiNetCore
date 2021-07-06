import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Role, User, UserLocal } from './common/authModels';
import { AuthenticationService } from './sevices/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})

export class AppComponent {
  currentUser: UserLocal;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
  }

  get isAdmin() {
    return this.currentUser  && this.currentUser.role === Role.Admin ;
  }

  logout() {
    this.authenticationService.logout().subscribe(x => {
      this.router.navigate(['/login']);
    });
  }
}
