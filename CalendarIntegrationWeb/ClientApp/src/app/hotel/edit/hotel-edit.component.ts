import { Component } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { FormValidation} from "../../shared/formvalidation";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Hotel} from "../../shared/models/hotel.model";

@Component({
  selector: 'app-hotel-edit',
  templateUrl: './hotel-edit.component.html',
})
export class HotelEditComponent {
  public hotelId: number;
  public hotel: Hotel;
  public get formControls(){
    return this.form.controls;
  }
  public form: FormGroup = new FormGroup({
    HotelCode: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Login: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Password: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Name: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Id: new FormControl('')
  });

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) {
    this.hotelId = this.route.snapshot.params.id;
  }

  public onSubmit(){
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.http.post('api/Hotel/Update', this.form.value, httpOptions).subscribe(
      () => this.router.navigate(['/']));
  }

  public ngOnInit() {
    this.http.get(`api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => {
        this.hotel = data;
        this.form.setValue({
          HotelCode: data.hotelCode,
          Login: data.login,
          Password: data.password,
          Name: data.name,
          Id: data.id,
        })
      });
  }
}
