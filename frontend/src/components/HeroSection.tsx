import heroImage from '../assets/pilt.jpg';

const HeroSection = () => {
  return (
    <div className="card my-4 border-0">
      <div className="row g-0">
        <div className="col-md-5 bg-primary text-white p-5">
          <p className="lead">
            Sed nec elit vestibulum, <strong>tincidunt orci</strong> et, sagittis ex. Vestibulum rutrum <strong>neque suscipit</strong> ante mattis maximus. Nulla non sapien <strong>viverra</strong>, <strong>lobortis lorem non</strong>, accumsan metus.
          </p>
        </div>
        <div className="col-md-7">
          <img src={heroImage} className="img-fluid w-100 h-100" alt="Bench under a tree" style={{ objectFit: 'cover' }} />
        </div>
      </div>
    </div>
  );
};

export default HeroSection;