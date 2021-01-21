import { Directive } from '@angular/core';
import { AbstractControl, FormGroup, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

@Directive({
  selector: '[appConfirmPassword]',
  providers: [{ provide: NG_VALIDATORS, useExisting: ConfirmPasswordDirective, multi: true }]
})
export class ConfirmPasswordDirective implements Validator {

  constructor() { }

  validate(control: AbstractControl) {
    const password = control.get('password');
    const confirm = control.get('confirmPassword');
    
    return password && confirm && password.value !== confirm.value ? { noMatch: true } : null;
  }

}
