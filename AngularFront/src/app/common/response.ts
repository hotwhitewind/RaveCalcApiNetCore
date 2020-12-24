import { User } from "./authModels";
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

export class UserResponse{
  error: boolean;
  result: User;
}
