import { HttpClient } from "@angular/common/http";
import {Component, Input} from '@angular/core';

@Component({
  selector: 'breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
})
export class BreadcrumbsComponent {
  @Input() menuItemLinks: Array<Array<String>>;

  getName(menuItemPair: Array<string>): string{
    return menuItemPair[0];
  }

  getUrl(menuItemPair: Array<string>): string{
    return menuItemPair[1];
  }
}
