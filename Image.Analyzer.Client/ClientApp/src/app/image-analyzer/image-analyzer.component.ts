import { Component } from '@angular/core';
import { ImageAnalysis, MachineLearningService } from '../openapi';
import _ from "lodash";

@Component({
  selector: 'app-image-analyzer',
  templateUrl: './image-analyzer.component.html',
})
export class ImageAnalyzerComponent {
  public imageUrl: string = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b1/VAN_CAT.png/220px-VAN_CAT.png";
  public analysisResult: ImageAnalysis = {};

  /**
   *
   */
  constructor(public apiGateway: MachineLearningService) {
  }

  public analyzeImage()
  {
    this.apiGateway.run(this.imageUrl).subscribe((analysis) => 
    {
      this.analysisResult = analysis;
    });
  }
  
}
