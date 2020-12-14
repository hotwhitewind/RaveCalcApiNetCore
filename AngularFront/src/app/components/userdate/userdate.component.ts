import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbCalendar, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { of } from 'rxjs';
import { City, CountriesResponse, Country, CountryInfoResponse, District, State } from 'src/app/common/response';
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
  currentCountry: Country;

  currentStates: State[];
  currentDistricts: District[];
  currentCities: City[];

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

    this.dataForm.get("city").valueChanges
      .subscribe(f => {
        this.onCityChanged(f);
      });

    of((this.http.getCountries()).subscribe((data: CountriesResponse) => {
      this.countries = data.result;
    }));
  }

  setDefalut() {
    this.currentCities = null;
    this.currentStates = null;
    this.currentDistricts = null;
    this.dataForm.get('state').patchValue(null);
    this.dataForm.get('district').patchValue(null);
    this.dataForm.get('city').patchValue(null);
  }

  onCountryChanged(e) {
    this.http.getCountryInfo(e).subscribe((data: CountryInfoResponse) => {
      this.setDefalut();
      this.currentCountry = data.result;
      this.currentStates = data.result?.states;
      this.currentCities = data.result?.cities;
    });
  }

  onStateChanged(e) {
    console.log(e)
    this.dataForm.get('district').patchValue(null);
    this.dataForm.get('city').patchValue(null);

    if (e == null) {
      this.currentDistricts = null;
      this.currentCities = this.currentCountry?.cities;
    }
    else {
      if (this.currentStates) {
        this.currentDistricts = this.currentStates.find(c => c.stateName == e)?.districts;
        this.currentCities = this.currentStates.find(c => c.stateName == e)?.cities;
      }
    }
  }

  onDistrictChanged(e) {
    console.log(e)
    this.dataForm.get('city').patchValue(null);
    if (e == null && this.currentStates != null) {
      let currentStateName = this.dataForm.get('state').value;
      this.currentCities = this.currentStates.find(c => c.stateName == currentStateName)?.cities;
    }
    else {
      if (this.currentDistricts) {
        this.currentCities = this.currentDistricts.find(c => c.districtName == e)?.cities;
      }
    }
  }

  onCityChanged(e) {

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
