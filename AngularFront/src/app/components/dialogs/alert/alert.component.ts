import { Component, ViewEncapsulation, OnInit, Inject, Input } from '@angular/core';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./alert.component.scss']
})

export class AlertComponent implements OnInit {
  messages: string[];
  constructor() { }

  ngOnInit(): void {
  }

}
