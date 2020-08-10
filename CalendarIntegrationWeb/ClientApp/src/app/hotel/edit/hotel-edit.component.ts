import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hotel-edit',
  templateUrl: './hotel-edit.component.html',
})
export class HotelEditComponent {
  public hotelId;

  constructor(private route: ActivatedRoute) {
    this.hotelId = this.route.snapshot.params.id;
  }
}
