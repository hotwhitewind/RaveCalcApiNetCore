import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { NgbCalendar, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateCustomParserFormatter } from '../../filters/dateformat';

@Component({
  selector: 'app-userdate',
  templateUrl: './userdate.component.html',
  styleUrls: ['./userdate.component.scss'],
  providers: [
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
  ]
})
export class UserdateComponent implements OnInit {

  title = 'RaveCalcApiAngular';
  modelDate: NgbDateStruct;
  private _dataForm: FormGroup;
  public get dataForm(): FormGroup {
    return this._dataForm;
  }
  public set dataForm(value: FormGroup) {
    this._dataForm = value;
  }

  response: Response;
  private url = '/apiv2';

  constructor(private fb: FormBuilder, private calendar: NgbCalendar, private http: HttpClient) { }

  ngOnInit() {
    this.dataForm = this.fb.group({
      country: '',
      state: '',
      city: '',
      birthdate: this.calendar.getToday(),
      birthtime: { hour: 0, minute: 0 }
    });
  }

  onSubmit(form: FormGroup) {
    console.log('Valid?', form.valid); // true or false
    console.log('Country', form.value.country);
    console.log('State', form.value.state);
    console.log('City', form.value.city);
    console.log('Birthdate', form.value.birthdate);
    console.log('BirthTime', form.value.birthtime);
    console.log(form.value);
    this.http.get(this.url + '/getallcountries').subscribe((data: Response) => {
      this.response = data;
      console.log(this.response);
    });
  }
}
