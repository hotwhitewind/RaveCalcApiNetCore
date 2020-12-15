import { BooleanLiteral } from "typescript";
import { Country } from "./model";

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

export class RaveResponse {
  error: boolean;
  result: string;
}
