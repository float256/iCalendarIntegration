import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {HttpClient} from "@angular/common/http";
import { HttpHeaders } from "@angular/common/http";
import { Router} from "@angular/router";
import { FormGroup, FormControl, Validators} from '@angular/forms';

import { Hotel } from "../../shared/models/hotel.model";
import { FormValidation } from "../../shared/formvalidation";

@Component({
  selector: 'app-hotel-add',
  templateUrl: './hotel-add.component.html',
})
export class HotelAddComponent {
  public get formControls(){
    return this.form.controls;
  }
  public form = new FormGroup({
    HotelCode: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Login: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Password: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Name: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation])
  });

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) { }

  public onSubmit(){
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.http.post('api/Hotel/Add', this.form.value, httpOptions).subscribe(
      (data: Hotel) => this.router.navigate([`/Hotel/${data.id}`]));
  }
}
