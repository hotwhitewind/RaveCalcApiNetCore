export class CountriesResponse {
  error: boolean;
  result: string[];
}

export class StatesResponse {
  error: boolean;
  result: string[];
}

export class CitiesResponse {
  error: boolean;
  result: string[];
}

export class CountryInfoResponse {
  error: boolean;
  result: Country;
}

export class City {
  cityAsciiName: string;
  cityName: string;
  latitude: number;
  longitude: number;
  countryCode: string;
  adminCode1: string;
  adminCode2: string;
  timeZone: string;
}

export class District {
  districtName: string;
  districtAsciiName: string;
  districtCode: string;
  cities: City[];
}

export class State {
  stateName: string;
  stateAsciiName: string;
  stateCode: string;
  districts: District[];
  cities: City[];
}

export class Country {
  countryName: string;
  countryISOCode: string;
  states: State[];
  cities: City[];
}
