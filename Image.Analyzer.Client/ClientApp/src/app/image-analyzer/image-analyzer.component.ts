import { Component } from '@angular/core';
import { ImageAnalysis, MachineLearningService } from '../openapi';

@Component({
  selector: 'app-image-analyzer',
  templateUrl: './image-analyzer.component.html',
})
export class ImageAnalyzerComponent {
  public imageUrl: string;
  public analysisResult: ImageAnalysis;

  /**
   *
   */
  constructor(public apiGateway: MachineLearningService) {
  }

  public analyzeImage()
  {
    const imageAnalysis$ = this.apiGateway.run(this.imageUrl).subscribe((analysis) => 
    {
      this.analysisResult = analysis;
    });
  }
  
}
