import { Component } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Hotel} from "../../shared/models/hotel.model";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {FormValidation} from "../../shared/formvalidation";
import {Room} from "../../shared/models/room.model";
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Component({
  selector: 'app-room-add',
  templateUrl: './room-edit.component.html',
})
export class RoomEditComponent {
  public hotel: Hotel;
  public room: Room;
  public get formControls(){
    return this.form.controls;
  }
  public form = new FormGroup({
    HotelId: new FormControl(),
    TLApiCode: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Url: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Name: new FormControl('', [Validators.required, FormValidation.onlySpacesValidation]),
    Id: new FormControl()
  });

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) { }

  public ngOnInit() {
    this.http.get(`api/Room/${this.route.snapshot.params.id}`).subscribe(
      (data: Room) => {
        this.room = data;
        this.form.setValue({
          HotelId: data.hotelId,
          TLApiCode: data.tlApiCode,
          Url: data.url,
          Name: data.name,
          Id: data.id
        });
        this.http.get(`api/Hotel/${this.room.hotelId}`).subscribe(
          (data: Hotel) => this.hotel = data);
      });
  }

  public onSubmit(){
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.http.post('api/Room/Update', this.form.value, httpOptions).subscribe(
      () => this.router.navigate([`/Hotel/${this.hotel.id}`]));
  }
}
