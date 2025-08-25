const Footer = () => {
  return (
    <footer className="footer">
      <div className="container">
        <div className="row">
          <div className="col-md-3">
            <h5>Curabitur</h5>
            <ul className="list-unstyled">
              <li><a href="#" className="text-white text-decoration-none">Emauris</a></li>
              <li><a href="#" className="text-white text-decoration-none">kfringilla</a></li>
              <li><a href="#" className="text-white text-decoration-none">Oin magna sem</a></li>
              <li><a href="#" className="text-white text-decoration-none">Kelementum</a></li>
            </ul>
          </div>
          <div className="col-md-3">
            <h5>Fusce</h5>
            <ul className="list-unstyled">
              <li><a href="#" className="text-white text-decoration-none">Econsectetur</a></li>
              <li><a href="#" className="text-white text-decoration-none">Ksollicitudin</a></li>
              <li><a href="#" className="text-white text-decoration-none">Omvulputate</a></li>
              <li><a href="#" className="text-white text-decoration-none">Nunc fringilla tellu</a></li>
            </ul>
          </div>
          <div className="col-md-6">
            <h5 className="text-start">Kontakt</h5>
            <div className="row">
              <div className="col-sm-6">
                <strong>Peakontor: Tallinnas</strong><br />
                Väike-Ameerika 1, 11415 Tallinn<br />
                Telefon: 605 4450<br />
                Faks: 605 3186
              </div>
              <div className="col-sm-6">
                <strong>Harukontor: Võrus</strong><br />
                Oja tn 7 (külastusaadress)<br />
                Telefon: 605 3330<br />
                Faks: 605 3155
              </div>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;