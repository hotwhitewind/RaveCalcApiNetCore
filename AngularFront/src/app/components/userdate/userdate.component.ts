import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ITS_JUST_ANGULAR } from '@angular/core/src/r3_symbols';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { NgbCalendar, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CountriesResponse } from 'src/app/common/response';
import { NgbDateCustomParserFormatter } from '../../../filters/dateformat';
import { FillselectService } from '../../sevices/fillselect.service';

@Component({
  selector: 'app-userdate',
  templateUrl: './userdate.component.html',
  styleUrls: ['./userdate.component.scss'],
  providers: [
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
    /*  { provide: FillselectService } */
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

  countries: string[];

  constructor(private fb: FormBuilder, private calendar: NgbCalendar, private http: FillselectService, private http1: HttpClient) { }

  ngOnInit() {
    this.dataForm = this.fb.group({
      country: [''],
      state: [''],
      city: [''],
      birthdate: this.calendar.getToday(),
      birthtime: { hour: 0, minute: 0 }
    });

    of((this.http.getCountries()).subscribe((data: CountriesResponse) => {
      this.countries = data.result;
      this.dataForm.controls.country.patchValue(this.countries[0]);
    }));
  }

  onSubmit(form: FormGroup) {
    console.log('Valid?', form.valid); // true or false
    console.log('Country', form.value.country);
    console.log('State', form.value.state);
    console.log('City', form.value.city);
    console.log('Birthdate', form.value.birthdate);
    console.log('BirthTime', form.value.birthtime);
    console.log(form.value);
/*     this.http.getCountries().subscribe((data: CountriesResponse) => {
      this.response = data;
      this.countries = data.result;
      console.log(this.response);
    });
 */  }
}
