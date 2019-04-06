import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
})
export class ToolbarComponent implements OnInit {
  @HostBinding('class.fixed') fixed = true;
  constructor() {}

  ngOnInit() {}
}
