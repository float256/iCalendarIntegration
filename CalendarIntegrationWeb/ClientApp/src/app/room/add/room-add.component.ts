import { Component } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {FormValidation} from "../../shared/formvalidation";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Hotel} from "../../shared/models/hotel.model";
import {Room} from "../../shared/models/room.model";

@Component({
  selector: 'app-room-add',
  templateUrl: './room-add.component.html',
})
export class RoomAddComponent {
  public hotelId: number;
  public hotel: Hotel;
  public get formControls(){
    return this.form.controls;
  }
  public form = new FormGroup({
    HotelId: new FormControl(),
    TLApiCode: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Url: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Name: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
  });

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) {
    this.hotelId = Number(this.route.snapshot.params.id);
  }

  public ngOnInit() {
    this.http.get(`api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data);
  }

  public onSubmit(){
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.form.value.HotelId = this.hotelId;
    this.http.post('api/Room/Add', this.form.value, httpOptions).subscribe(
      (data: Room) => this.router.navigate([`/Hotel/${data.hotelId}`]));
  }
}
