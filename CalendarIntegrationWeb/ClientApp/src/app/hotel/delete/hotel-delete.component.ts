import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hotel-delete',
  templateUrl: './hotel-delete.component.html',
})
export class HotelDeleteComponent {
  public hotelId;

  constructor(private route: ActivatedRoute) {
    this.hotelId = this.route.snapshot.params.id;
  }
}
