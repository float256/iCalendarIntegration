import {AbstractControl, ValidationErrors} from "@angular/forms";

export class FormValidation {
  public static onlySpacesValidation(control: AbstractControl): ValidationErrors | null {
    let validatedValue = control.value as string;
    if ((validatedValue.length > 0) && (validatedValue.trim().length === 0)) {
      return {onlySpaces: true}
    } else {
      return null;
    }
  }

  public static urlValidation(control: AbstractControl): ValidationErrors | null {
    const regexpUrlExpression = new RegExp('^(https?:\\/\\/)?' +
      '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' +
      '((\\d{1,3}\\.){3}\\d{1,3}))' +
      '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' +
      '(\\?[;&a-z\\d%_.~+=-]*)?' +
      '(\\#[-a-z\\d_]*)?$','i');
    let url: string = control.value as string;

    if (regexpUrlExpression.test(url)) {
      return null;
    } else {
      return { incorrectUrl: true };
    }
  }
}
