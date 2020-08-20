import {AbstractControl, ValidationErrors} from "@angular/forms";

export class FormValidation{
  public static onlySpacesValidation(control: AbstractControl) : ValidationErrors | null {
    let validatedValue = control.value as string;
    if((validatedValue.length > 0) && (validatedValue.trim().length === 0)){
      return {onlySpaces: true}
    } else {
      return null;
    }
  }
}
