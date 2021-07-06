import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';
import { AlertComponent } from '../components/dialogs/alert/alert.component';

@Injectable({ providedIn: 'root' })
export class DialogService {

  /**
   *
   */
  constructor(private dialog: NgbModal) {

  }

  alert(messages: string[]) {
    const modalRef = this.dialog.open(AlertComponent);
    modalRef.componentInstance.messages = messages;
  }
}
