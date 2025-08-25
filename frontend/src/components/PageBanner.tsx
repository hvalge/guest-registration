import React from 'react';
import bannerImage from '../assets/libled.jpg';

interface PageBannerProps {
  title: string;
}

const PageBanner: React.FC<PageBannerProps> = ({ title }) => {
  return (
    <div className="row g-0 mb-4" style={{ height: '100px' }}>
      <div className="col-md-3 bg-primary text-white d-flex align-items-center p-4">
        <h2 className="mb-0">{title}</h2>
      </div>
      <div 
        className="col-md-9 h-100"
        style={{
          backgroundImage: `url(${bannerImage})`,
          backgroundSize: 'cover',
          backgroundPosition: 'center center'
        }}
      >
      </div>
    </div>
  );
};

export default PageBanner;
