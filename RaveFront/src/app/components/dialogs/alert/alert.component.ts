import { Component, ViewEncapsulation, OnInit, Inject, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./alert.component.scss']
})

export class AlertComponent implements OnInit {
  messages: string[];
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

}
