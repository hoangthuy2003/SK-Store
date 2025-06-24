import { Component, OnInit, signal, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgApexchartsModule, ChartComponent } from 'ng-apexcharts';
import {
  ApexChart, ApexAxisChartSeries, ApexNonAxisChartSeries, ApexDataLabels, ApexPlotOptions,
  ApexXAxis, ApexTitleSubtitle, ApexLegend, ApexFill, ApexTooltip
} from 'ng-apexcharts';

import { DashboardService } from '../../../services/dashboard.service';
import { DashboardStats } from '../../../models/dashboard.model';
import { VndCurrencyPipe } from '../../../pipes/vnd-currency.pipe';
import { UtilsService } from '../../../services/utils.service';

// SỬA LẠI TYPE NÀY ĐỂ LINH HOẠT HƠN
export type ChartOptions = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  title: ApexTitleSubtitle;
  legend: ApexLegend;
  fill: ApexFill;
  tooltip: ApexTooltip;
  labels: any;
};

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule, VndCurrencyPipe, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  public utilsService = inject(UtilsService);
  private vndPipe = new VndCurrencyPipe(); // Khởi tạo pipe để dùng trong code

  isLoading = signal(true);
  stats = signal<DashboardStats | null>(null);

  revenueChartOptions = signal<Partial<ChartOptions> | null>(null);
  categoryChartOptions = signal<Partial<ChartOptions> | null>(null);

  ngOnInit(): void {
    this.dashboardService.getStatistics().subscribe(data => {
      this.stats.set(data);
      this.setupCharts(data);
      this.isLoading.set(false);
    });
  }

  private setupCharts(data: DashboardStats): void {
    // Revenue Chart
    this.revenueChartOptions.set({
      series: [{
        name: "Doanh thu",
        data: data.revenueOverTime.map(item => item.revenue)
      }],
      chart: { type: "area", height: 350, toolbar: { show: false } },
      xaxis: {
        type: "datetime",
        categories: data.revenueOverTime.map(item => item.date)
      },
      dataLabels: { enabled: false },
      tooltip: {
        x: { format: 'dd/MM/yyyy' },
        y: {
          // SỬA LẠI CÁCH DÙNG PIPE
          formatter: (val) => this.vndPipe.transform(val)
        }
      },
    });

    // Category Chart
    this.categoryChartOptions.set({
      series: data.categorySalesDistribution.map(item => item.totalQuantitySold),
      chart: { type: "donut", height: 350 },
      labels: data.categorySalesDistribution.map(item => item.categoryName),
      legend: { position: 'bottom' },
      plotOptions: { pie: { donut: { labels: { show: true, total: { show: true, label: 'Tổng SP' } } } } }
    });
  }
}