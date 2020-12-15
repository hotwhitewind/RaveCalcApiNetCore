import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbCalendar, NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct, NgbTimeAdapter } from '@ng-bootstrap/ng-bootstrap';
import { of } from 'rxjs';
import { CountriesResponse, CountryInfoResponse, RaveResponse } from 'src/app/common/response';
import { City, Country, District, State } from 'src/app/common/model';
import { NgbDateCustomAdapter, NgbDateCustomParserFormatter } from '../../../filters/dateformat';
import { FillselectService } from '../../sevices/fillselect.service';
import { NgbTimeStringAdapter } from 'src/filters/timeformat';

@Component({
  selector: 'app-userdate',
  templateUrl: './userdate.component.html',
  styleUrls: ['./userdate.component.scss'],
  providers: [
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
    { provide: NgbDateAdapter, useClass: NgbDateCustomAdapter },
    { provide: NgbTimeAdapter, useClass: NgbTimeStringAdapter },
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
  currentCity: City;
  currentStates: State[];
  currentDistricts: District[];
  currentCities: City[];
  currentJsonResponce: string;

  constructor(private fb: FormBuilder, private calendar: NgbCalendar, private http: FillselectService,
    private dateAdapter: NgbDateAdapter<string>, private timeAdapter: NgbTimeAdapter<string>) { }

  ngOnInit() {
    this.currentJsonResponce = null;
    this.dataForm = this.fb.group({
      country: [null],
      state: [null],
      district: [null],
      city: [null],
      birthdate: this.dateAdapter.toModel(this.calendar.getToday()),
      birthtime: '00:00'
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

    of(this.http.getCountries().subscribe((data: CountriesResponse) => {
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
    if (e == null) {
      this.currentCity = null;
    }
    else {
      this.currentCity = this.currentCities.find(c => c.cityName == e);
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
    this.http.getRaveJson(form.value.birthdate,
      form.value.birthtime, "Europe/Moscow").subscribe((data: RaveResponse) => {
        this.currentJsonResponce = data.result;
        console.log(this.currentJsonResponce);
      })
  }
}
