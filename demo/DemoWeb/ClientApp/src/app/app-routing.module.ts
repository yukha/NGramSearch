import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HelpComponent } from './help/help.component';
import { SearchResultsComponent } from './search-results/search-results.component';
import { SearchSourceType } from './search-source-type.enum';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'actors',
  },
  {
    path: 'actors',
    component: SearchResultsComponent,
    data: { searchSourceType: SearchSourceType.actors },
  },
  {
    path: 'films',
    component: SearchResultsComponent,
    data: { searchSourceType: SearchSourceType.films },
  },
  {
    path: 'help',
    component: HelpComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
