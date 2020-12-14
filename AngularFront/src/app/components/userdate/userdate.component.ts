import { HttpClient } from '@angular/common/http';
import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { ITS_JUST_ANGULAR } from '@angular/core/src/r3_symbols';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { NgbCalendar, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CitiesResponse, City, CountriesResponse, Country, CountryInfoResponse, District, State, StatesResponse } from 'src/app/common/response';
import { NgbDateCustomParserFormatter } from '../../../filters/dateformat';
import { FillselectService } from '../../sevices/fillselect.service';

@Component({
  selector: 'app-userdate',
  templateUrl: './userdate.component.html',
  styleUrls: ['./userdate.component.scss'],
  providers: [
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
    { provide: FillselectService }
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
  states: string[];
  districts: string[];
  cities: string[];

  currentCountry: Country;
  currentState: State;
  currentDistrict: District;
  currentCity: City;

  constructor(private fb: FormBuilder, private calendar: NgbCalendar, private http: FillselectService) { }

  ngOnInit() {
    this.dataForm = this.fb.group({
      country: [null],
      state: [null],
      district: [null],
      city: [null],
      birthdate: this.calendar.getToday(),
      birthtime: { hour: 0, minute: 0 }
    });

    this.dataForm.get("country").valueChanges
      .subscribe(f => {
        this.onCountryChanged(f);
      });

    this.dataForm.get("state").valueChanges
      .subscribe(f => {
        this.onStateChanged(f);
      });

    this.dataForm.get("district").valueChanges
      .subscribe(f => {
        this.onDistrictChanged(f);
      });

    of((this.http.getCountries()).subscribe((data: CountriesResponse) => {
      this.countries = data.result;
    }));
  }

  setDefalut() {
    this.currentCountry = null;
    this.currentState = null;
    this.currentDistrict = null;
    this.currentCity = null;
    this.states = null;
    this.districts = null;
    this.cities = null;
    this.dataForm.get("state").patchValue(null);
    this.dataForm.get("district").patchValue(null);
    this.dataForm.get("city").patchValue(null);
  }

  onCountryChanged(e) {
    this.http.getCountryInfo(e).subscribe((data: CountryInfoResponse) => {
      this.setDefalut();
      this.currentCountry = data.result;
      if (data.result.states && data.result.states.length > 0)
        this.states = data.result.states.map(s => s.stateName);
      if (data.result.cities && data.result.cities.length > 0)
        this.cities = data.result.cities.map(s => s.cityName);
    })
  }

  onStateChanged(e) {
    console.log(e)
    if (this.currentCountry && e) {
      this.currentState = this.currentCountry.states.find(c => c.stateName === e);
      if (this.currentState) {
        if (this.currentState.districts && this.currentState.districts.length > 0) {
          this.districts = this.currentState.districts.map(s => s.districtName);
        }
        if (this.currentState.cities && this.currentState.cities.length > 0) {
          this.cities = this.currentState.cities.map(s => s.cityName);
        }
      }
    }
  }

  onDistrictChanged(e) {
    console.log(e)
    if (this.currentState && e) {
      if (this.currentState.districts && this.currentState.districts.length > 0)
        this.currentDistrict = this.currentState.districts.find(c => c.districtName === e);
      if (this.currentDistrict && this.currentDistrict.cities && this.currentDistrict.cities.length > 0)
        this.cities = this.currentDistrict.cities.map(s => s.cityName);
    }
  }

  onSubmit(form: FormGroup) {
    console.log('Valid?', form.valid); // true or false
    console.log('Country', form.value.country);
    console.log('State', form.value.state);
    console.log('City', form.value.city);
    console.log('Birthdate', form.value.birthdate);
    console.log('BirthTime', form.value.birthtime);
    console.log(form.value);
  }
}
